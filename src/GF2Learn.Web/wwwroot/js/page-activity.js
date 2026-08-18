(function (global) {
  "use strict";

  var HEARTBEAT_MS = 15000;
  var IDLE_AFTER_MS = 60000;
  var state = null;

  function meta(name) {
    var element = document.querySelector('meta[name="' + name + '"]');
    return element ? element.getAttribute("content") || "" : "";
  }

  function apiUrl(path) {
    var pathBase = meta("gf2-path-base").replace(/\/$/, "");
    return pathBase + "/" + path.replace(/^\//, "");
  }

  function newSessionId() {
    if (global.crypto && typeof global.crypto.randomUUID === "function") {
      return global.crypto.randomUUID();
    }

    var bytes = new Uint8Array(16);
    global.crypto.getRandomValues(bytes);
    bytes[6] = (bytes[6] & 0x0f) | 0x40;
    bytes[8] = (bytes[8] & 0x3f) | 0x80;
    var hex = Array.from(bytes, function (value) {
      return value.toString(16).padStart(2, "0");
    }).join("");
    return hex.slice(0, 8) + "-" + hex.slice(8, 12) + "-" + hex.slice(12, 16) + "-" +
      hex.slice(16, 20) + "-" + hex.slice(20);
  }

  function findContent() {
    return document.querySelector("[data-activity-content-type][data-activity-content-slug]");
  }

  function markInteraction() {
    if (state) state.lastInteractionAt = Date.now();
  }

  function isActivelyReading() {
    return !!state &&
      document.visibilityState === "visible" &&
      document.hasFocus() &&
      Date.now() - state.lastInteractionAt <= IDLE_AFTER_MS;
  }

  function tick() {
    if (isActivelyReading()) state.activeSeconds += 1;
  }

  function sendHeartbeat(keepalive) {
    var current = state;
    if (!current || current.activeSeconds <= current.lastAcknowledgedSeconds || current.sending) {
      return Promise.resolve();
    }

    var token = meta("request-verification-token");
    if (!token) return Promise.resolve();

    var snapshot = current.activeSeconds;
    current.sending = true;
    return fetch(apiUrl("/api/activity/heartbeat"), {
      method: "POST",
      credentials: "same-origin",
      keepalive: !!keepalive,
      headers: {
        "Content-Type": "application/json",
        "RequestVerificationToken": token
      },
      body: JSON.stringify({
        sessionId: current.sessionId,
        contentType: current.contentType,
        contentSlug: current.contentSlug,
        activeSeconds: snapshot,
        startedAt: current.startedAt
      })
    }).then(function (response) {
      if (response.ok) {
        current.lastAcknowledgedSeconds = Math.max(current.lastAcknowledgedSeconds, snapshot);
      }
    }).catch(function () {
      // The next cumulative heartbeat safely retries this time.
    }).finally(function () {
      current.sending = false;
    });
  }

  function stop(flush) {
    if (!state) return;
    if (flush) sendHeartbeat(true);
    global.clearInterval(state.tickTimer);
    global.clearInterval(state.heartbeatTimer);
    state = null;
  }

  function start() {
    stop(true);
    if (meta("gf2-authenticated") !== "true" || meta("gf2-activity-enabled") !== "true") return;

    var content = findContent();
    if (!content) return;

    var contentType = content.getAttribute("data-activity-content-type");
    var contentSlug = content.getAttribute("data-activity-content-slug");
    if (!contentType || !contentSlug) return;

    state = {
      sessionId: newSessionId(),
      contentType: contentType,
      contentSlug: contentSlug,
      startedAt: new Date().toISOString(),
      activeSeconds: 0,
      lastAcknowledgedSeconds: 0,
      lastInteractionAt: Date.now(),
      sending: false,
      tickTimer: global.setInterval(tick, 1000),
      heartbeatTimer: global.setInterval(function () { sendHeartbeat(false); }, HEARTBEAT_MS)
    };
  }

  ["pointerdown", "keydown", "scroll", "touchstart"].forEach(function (eventName) {
    global.addEventListener(eventName, markInteraction, { passive: true });
  });

  document.addEventListener("visibilitychange", function () {
    if (document.visibilityState === "hidden") sendHeartbeat(true);
    else markInteraction();
  });
  global.addEventListener("pagehide", function () { sendHeartbeat(true); });
  global.addEventListener("focus", markInteraction);
  global.addEventListener("gf2-enhanced-nav", function () {
    global.requestAnimationFrame(start);
  });

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start);
  } else {
    start();
  }
})(window);
