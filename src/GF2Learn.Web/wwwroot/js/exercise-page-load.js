(function (global) {
  var POLL_MS = 80;
  var TIMEOUT_MS = 20000;
  var EMPTY_HOSTS_GRACE_MS = 12000;
  var pollTimer = null;
  var timeoutTimer = null;
  var emptyGraceTimer = null;
  var hostObserver = null;

  function pendingPage() {
    return document.querySelector(".exercise-page--pending[data-exercise-interactive]");
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

  function markReady(page) {
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
    if (timeoutTimer) {
      clearTimeout(timeoutTimer);
      timeoutTimer = null;
    }
    if (emptyGraceTimer) {
      clearTimeout(emptyGraceTimer);
      emptyGraceTimer = null;
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
      return;
    }

    var hosts = hostsIn(page);
    if (hosts.length === 0) return;

    if (emptyGraceTimer) {
      clearTimeout(emptyGraceTimer);
      emptyGraceTimer = null;
    }

    if (allEditorsReady(page)) markReady(page);
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
    var page = pendingPage();
    if (!page) return;

    page.setAttribute("aria-busy", "true");
    watchForHosts(page);
    checkReady();

    if (!pollTimer) {
      pollTimer = setInterval(checkReady, POLL_MS);
    }

    if (!timeoutTimer) {
      timeoutTimer = setTimeout(function () {
        var p = pendingPage();
        if (!p) return;
        p.classList.add("exercise-page--timeout");
        markReady(p);
      }, TIMEOUT_MS);
    }

    if (!emptyGraceTimer) {
      emptyGraceTimer = setTimeout(function () {
        var p = pendingPage();
        if (!p) return;
        if (hostsIn(p).length === 0) markReady(p);
      }, EMPTY_HOSTS_GRACE_MS);
    }
  }

  global.gf2ExercisePage = {
    checkReady: checkReady,
    startWatching: startWatching
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
