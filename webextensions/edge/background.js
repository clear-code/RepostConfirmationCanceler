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
  let i = 0;
  let j = 0;
  let mp, cp;

  while ((j < string.length) && (wild[i] != '*')) {
    if ((wild[i] != string[j]) && (wild[i] != '?')) {
      return 0;
    }
    i += 1;
    j += 1;
  }
  while (j < string.length) {
    if (wild[i] == '*') {
      i += 1;

      if (i == wild.length) {
        return 1;
      }
      mp = i;
      cp = j + 1
    } else if ((wild[i] == string[j]) || (wild[i] == '?')) {
      i += 1;
      j += 1;
    } else {
      i = mp;
      j = cp;
      cp += 1;
    }
  }
  while (wild[i] == '*' && i < wild.length) {
    i += 1;
  }
  return i >= wild.length;
};

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
   * Request redirection to Native Messaging Hosts.
   *
   * * chrome.tabs.get() is to confirm that the URL is originated from
   *   an actual tab (= not an internal prefetch request).
   *
   * * Request Example: "Q edge https://example.com/".
   */
  startMonitoring() {
    const query = new String('S ' + BROWSER);
    console.log(`Cenceler: Send "start monitoring" message: ${query}`);
    chrome.runtime.sendNativeMessage(SERVER_NAME, query);
  },

  match(section, url) {
    for (let pattern of (section.URLExcludePatterns || section.Excludes || [])) {
      if (Array.isArray(pattern)) {
        pattern = pattern[0];
      }
      if (wildcmp(pattern, url)) {
        console.log(`* Match Exclude ${section.Name} [${pattern}]`);
        return false;
      }
    }

    for (let pattern of (section.URLPatterns || section.Patterns || [])) {
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

  getBrowserName(section) {
    const name = section.Name.toLowerCase();

    /* Guess the browser name from the executable path */
    if (name.match(/^custom/i)) {
      if (section.Path.match(RegExp(BROWSER, 'i')))
        return BROWSER;
    }
    return name;
  },

  handleURL(config, url){
    if (!url) {
      console.log(`* Empty URL found`);
      return false;
    }

    if (!/^https?:/.test(url)) {
      console.log(`* Ignore non-HTTP/HTTPS URL ${url}`);
      return false;
    }
    const urlToMatch = config.IgnoreQueryString ? url.replace(/\?.*/, '') : url;

    console.log(`* Lookup sections for ${urlToMatch}`);
    for (const section of config.Sections) {
      console.log(`handleURL: check for section ${section.Name} (${JSON.stringify(section)})`);

      if (this.match(section, urlToMatch)) {
        console.log(` => unmatched`);
        this.startMonitoring();
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
        if(this.handleURL(config, url)){
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
    this.handleURL(config, url);
  },

  onNavigationCommitted(details) {
    const url = details.url;
    console.log(`onNavigationCommitted: ${url}`);
    const config = this.cached;
    this.handleURL(config, url);
  },
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

/* Tab book-keeping for intelligent tab handlings */
chrome.tabs.onUpdated.addListener(RepostConfirmationCancelerTalkClient.onTabUpdated.bind(RepostConfirmationCancelerTalkClient));
chrome.webNavigation.onCommitted.addListener(RepostConfirmationCancelerTalkClient.onNavigationCommitted.bind(RepostConfirmationCancelerTalkClient));

RepostConfirmationCancelerTalkClient.init();
