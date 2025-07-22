const assert = require('assert');
const sinon = require('sinon');
const chromestub = require('./chrome-stub.js')

describe('checkRedirectIntervalLimit', () => {
  const redirectors = [
    { browser: 'chrome', module: require('../testee/chrome.js') },
    { browser: 'edge',   module: require('../testee/edge.js')   },
  ].forEach(({browser, module}) => {
    describe(browser, () => {
      const repost_confirmation_canceler = module.client;
      const tabId = 123;
      let clock;

      beforeEach(() => {
        clock = sinon.useFakeTimers();
        repost_confirmation_canceler.recentRequests = {};
      });

      afterEach(() => {
        delete repost_confirmation_canceler.recentRequests;
        clock.restore();
      });

      it('allow first request', () => {
        const url = "https://www.google.com/";
        const ret = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        assert.equal(ret, false);
      });

      it('skip frequent same request', () => {
        const url = "https://www.google.com/";
        const ret1 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        clock.tick(300);
        const ret2 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        assert.deepEqual({ ret1: !!ret1, ret2: !!ret2 }, { ret1: false, ret2: true });
      });

      it('allow same request after 1 sec', () => {
        const url = "https://www.google.com/";
        const ret1 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        clock.tick(1001);
        const ret2 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        assert.deepEqual({ ret1: !!ret1, ret2: !!ret2 }, { ret1: false, ret2: false });
      });

      it('allow same request in different tab', () => {
        const url = "https://www.google.com/";
        const ret1 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId, url);
        clock.tick(300);
        const ret2 = repost_confirmation_canceler.checkRedirectIntervalLimit(tabId + 1, url);
        assert.deepEqual({ ret1: !!ret1, ret2: !!ret2 }, { ret1: false, ret2: false });
      });
    });
  });
});
