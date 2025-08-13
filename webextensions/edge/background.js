'use strict';

/*
 * Basic settings for modern browsers
 *
 * Programming Note: Just tweak these constants for each browser.
 * It should work fine across Edge, Chrome and Firefox without any
 * further modifications.
 */
const BROWSER = 'edge';
const SERVER_NAME = 'com.clear_code.repost_confirmation_canceler';
const ALARM_MINUTES = 0.5;
/*
 * RepostConfirmationCanceler's matching function
 *
 *  1. `?` represents a single character.
 *  2. `*` represents an arbitrary substring.
 *
 * >>> wildcmp("http?://*.example.com/*", "https://www.example.com/")
 * true
 */
function wildcmp(wild, string) {
  if(!wild || !string) {
    return false;
  }
  const pattern = wildcardToRegexp(wild);
  const regex = new RegExp(`^${pattern}$`, "i");
  return regex.test(string);
};

function wildcardToRegexp(source) {
  // https://stackoverflow.com/questions/6300183/sanitize-string-of-regex-characters-before-regexp-build
  const sanitized = source.replace(/[#-.]|[[-^]|[?|{}]/g, "\\$&");
  const wildcardAccepted = sanitized.replace(/\\\*/g, ".*").replace(/\\\?/g, ".");
  return wildcardAccepted;
}

/*
 * Observe WebRequests with config fetched from RepostConfirmationCanceler.
 *
 * A typical configuration looks like this:
 *
 * {
 *   Sections: [
 *     {Name:"edge", Patterns:["*://example.com/*"], Excludes:[]},
 *     ...
 *   ]
 * }
 */
const RepostConfirmationCancelerTalkClient = {
  cached: null,

  init() {
    this.cached = null;
    this.ensureLoadedAndConfigured();
    console.log('Running as RepostConfirmationCancelerTalkClient Talk client');
  },

  async ensureLoadedAndConfigured() {
    return this._promisedLoadedAndConfigured = this._promisedLoadedAndConfigured || Promise.all([
      !this.cached && this.configure()
    ]);
  },
  _promisedLoadedAndConfigured: null,

  async configure() {
    const query = new String('C ' + BROWSER);

    const resp = await chrome.runtime.sendNativeMessage(SERVER_NAME, query);
    if (chrome.runtime.lastError || !resp) {
      console.log('Cannot fetch config', query, JSON.stringify(chrome.runtime.lastError));
      return;
    }
    const isStartup = (this.cached == null);
    this.cached = resp.config;
    this.cached.NamedSections = Object.fromEntries(resp.config.Sections.map(section => [section.Name.toLowerCase(), section]));
    console.log('Fetch config', JSON.stringify(this.cached));

    if (isStartup) {
      this.handleStartup();
    }
  },

  /*
   * Request monitoring to Native Messaging Hosts.
   * * Request Example: "Q edge".
   */
  startMonitoring() {
    const query = new String('Q ' + BROWSER);
    console.log(`Cenceler: Send start monitoring message: ${query}`);
    chrome.runtime.sendNativeMessage(SERVER_NAME, query);
  },

  match(section, url) {
    for (let pattern of (section.Excludes || [])) {
      if (Array.isArray(pattern)) {
        pattern = pattern[0];
      }
      if (wildcmp(pattern, url)) {
        console.log(`* Match Exclude ${section.Name} [${pattern}]`);
        return false;
      }
    }

    for (let pattern of (section.Patterns || [])) {
      if (Array.isArray(pattern)) {
        pattern = pattern[0];
      }
      if (wildcmp(pattern, url)) {
        console.log(`* Match ${section.Name} [${pattern}]`);
        return true;
      }
    }
    return false;
  },

  handleURL(config, url, callbackWhenMatch){
    if (!url) {
      console.log(`* Empty URL found`);
      return false;
    }

    if (!/^https?:/.test(url)) {
      console.log(`* Ignore non-HTTP/HTTPS URL ${url}`);
      return false;
    }
    const urlToMatch = url;

    console.log(`* Lookup sections for ${urlToMatch}`);
    for (const section of config.Sections) {
      if (section.Name.toLowerCase() !== "targets")
      {
        continue;
      }
      console.log(`handleURL: check for section ${section.Name} (${JSON.stringify(section)})`);
      if (this.match(section, urlToMatch)) {
        console.log(` => unmatched`);
        callbackWhenMatch();
        return true;
      }
      else {
        console.log(` => unmatched`);
        continue;
      }
    }
    return false;
  },

  handleAllTabs() {
    const config = this.cached;
    console.log(`handleAllTabs`);
    chrome.tabs.query({  }).then(tabs => {
      for (const tab of tabs) {
        const url = tab.url ?? tab.pendingUrl;
        console.log(`handleAllTabs ${url} (tab=${tab.id})`);
        if(this.handleURL(config, url, this.startMonitoring)){
          break;
        }
      };
    });
  },

  handleStartup() {
    this.handleAllTabs();
  },

  async onTabUpdated(tabId, info, tab) {
    await this.ensureLoadedAndConfigured();

    const config = this.cached;
    const url = tab.pendingUrl || tab.url;
    this.handleURL(config, url, this.startMonitoring);
  },

  onNavigationCommitted(details) {
    const url = details.url;
    console.log(`onNavigationCommitted: ${url}`);
    const config = this.cached;
    this.handleURL(config, url, this.startMonitoring);
  },

  onErrorOccurred(details) {
    console.log('onErrorOccurred:', details);
    if (details.error === 'net::ERR_CACHE_MISS') {
      const url = details.url;
      const config = this.cached;
      const tabId = details.tabId;
      this.handleURL(config, url, () => { 
        this.closeTab(tabId);
      });
    }
  },

  closeTab(tabId) {
    if (tabId !== -1) {
      console.log("Closing tab:", tabId);
      chrome.tabs.remove(tabId, () => {
        if (chrome.runtime.lastError) {
          console.log("Error while closing tab:", chrome.runtime.lastError.message)
        } else {
          console.log("Tab closed");
        }
      });
    }
  }
};

/* Refresh config for every N minute */
console.log('Poll config for every', ALARM_MINUTES , 'minutes');
chrome.alarms.create('poll-config', {'periodInMinutes': ALARM_MINUTES});

chrome.alarms.onAlarm.addListener((alarm) => {
  if (alarm.name === 'poll-config') {
    RepostConfirmationCancelerTalkClient.configure();
    RepostConfirmationCancelerTalkClient.handleAllTabs();
    //handleURL for all url in tabs.
  }
});

chrome.webRequest.onErrorOccurred.addListener(
  RepostConfirmationCancelerTalkClient.onErrorOccurred.bind(RepostConfirmationCancelerTalkClient),
  {urls: ["<all_urls>"]}
);

/* Tab book-keeping for intelligent tab handlings */
chrome.tabs.onUpdated.addListener(RepostConfirmationCancelerTalkClient.onTabUpdated.bind(RepostConfirmationCancelerTalkClient));
chrome.webNavigation.onCommitted.addListener(RepostConfirmationCancelerTalkClient.onNavigationCommitted.bind(RepostConfirmationCancelerTalkClient));

RepostConfirmationCancelerTalkClient.init();
