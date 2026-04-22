'use strict';

/*
 * Basic settings for modern browsers
 *
 * Programming Note: Just tweak these constants for each browser.
 * It should work fine across Edge, Chrome and Firefox without any
 * further modifications.
 */
const BROWSER = 'edge';
const SERVER_NAME = 'com.clear_code.browser_startup_launcher';

// ブラウザ起動時（service worker開始時）に実行される
chrome.runtime.onStartup.addListener(() => {
  chrome.runtime.sendNativeMessage(
    SERVER_NAME,
    { command: `Q {BROWSER}` },
    (response) => {
      if (chrome.runtime.lastError) {
        console.error(chrome.runtime.lastError.message);
      }
    }
  );
});