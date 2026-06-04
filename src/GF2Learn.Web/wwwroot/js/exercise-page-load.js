(function (global) {
  var POLL_MS = 80;
  var TIMEOUT_MS = 45000;
  var pollTimer = null;
  var timeoutTimer = null;

  function pendingPage() {
    return document.querySelector(".exercise-page--pending[data-exercise-interactive]");
  }

  function hostsIn(page) {
    return page.querySelectorAll(".playground-editor-host");
  }

  function hostIsReady(host) {
    return (
      host.dataset.editorReady === "1" &&
      !host.classList.contains("playground-editor-host-loading") &&
      !!host.querySelector(".monaco-editor")
    );
  }

  function allEditorsReady(page) {
    var hosts = hostsIn(page);
    if (!hosts.length) return true;
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
  }

  function checkReady() {
    var page = pendingPage();
    if (!page) {
      stopWatching();
      return;
    }
    if (allEditorsReady(page)) markReady(page);
  }

  function startWatching() {
    var page = pendingPage();
    if (!page) return;

    page.setAttribute("aria-busy", "true");
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
  }

  global.gf2ExercisePage = {
    checkReady: checkReady,
    startWatching: startWatching
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", startWatching);
  } else {
    startWatching();
  }

  document.addEventListener("gf2-enhanced-nav", startWatching);
})(window);
