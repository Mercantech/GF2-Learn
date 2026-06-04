(function (global) {
  var POLL_MS = 100;
  var FORCE_READY_MS = 8000;

  var pollTimer = null;
  var forceTimer = null;
  var hostObserver = null;

  function pendingPage() {
    return document.querySelector(".exercise-page.exercise-page--pending");
  }

  function hostsIn(page) {
    return page.querySelectorAll(".playground-editor-host");
  }

  function hostIsReady(host) {
    if (!host) return false;
    if (host.dataset.editorReady === "1") return true;
    if (host.id && global.gf2Playground && global.gf2Playground.isReady(host.id)) {
      return true;
    }
    return (
      !!host.querySelector(".monaco-editor") &&
      !host.classList.contains("playground-editor-host-loading")
    );
  }

  function allEditorsReady(page) {
    var hosts = hostsIn(page);
    if (!hosts.length) return false;
    for (var i = 0; i < hosts.length; i++) {
      if (!hostIsReady(hosts[i])) return false;
    }
    return true;
  }

  function markReady(page, timedOut) {
    if (!page || !page.classList.contains("exercise-page--pending")) return;

    if (timedOut) page.classList.add("exercise-page--timeout");
    page.classList.remove("exercise-page--pending");
    page.classList.add("exercise-page--ready");
    page.removeAttribute("aria-busy");

    var loader = page.querySelector(".exercise-page-loader");
    if (loader) loader.setAttribute("aria-hidden", "true");

    stopWatching();
    global.dispatchEvent(new CustomEvent("gf2-exercise-page-ready"));
  }

  function stopWatching() {
    if (pollTimer) {
      clearInterval(pollTimer);
      pollTimer = null;
    }
    if (forceTimer) {
      clearTimeout(forceTimer);
      forceTimer = null;
    }
    if (hostObserver) {
      hostObserver.disconnect();
      hostObserver = null;
    }
  }

  function checkReady() {
    var page = pendingPage();
    if (!page) {
      stopWatching();
      return true;
    }
    if (allEditorsReady(page)) {
      markReady(page, false);
      return true;
    }
    return false;
  }

  function watchForHosts(page) {
    if (hostObserver) hostObserver.disconnect();
    var root = page.querySelector(".exercise-page-body") || page;
    hostObserver = new MutationObserver(function () {
      checkReady();
    });
    hostObserver.observe(root, { childList: true, subtree: true });
  }

  function startWatching() {
    stopWatching();

    var page = pendingPage();
    if (!page) return;

    page.setAttribute("aria-busy", "true");
    watchForHosts(page);
    if (checkReady()) return;

    pollTimer = setInterval(checkReady, POLL_MS);

    forceTimer = setTimeout(function () {
      var p = pendingPage();
      if (p) markReady(p, true);
    }, FORCE_READY_MS);
  }

  function release() {
    var page = pendingPage();
    if (page) markReady(page, false);
  }

  global.gf2ExercisePage = {
    checkReady: checkReady,
    startWatching: startWatching,
    release: release
  };

  function boot() {
    startWatching();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", boot);
  } else {
    boot();
  }

  document.addEventListener("gf2-enhanced-nav", boot);
})(window);
