(function (global) {
  /**
   * Bygger absolut API-URL (respekterer <base href> og PathBase).
   */
  function apiUrl(path) {
    var rel = path.charAt(0) === "/" ? path : "/" + path;
    var meta = document.querySelector('meta[name="gf2-path-base"]');
    var pathBase = meta ? meta.getAttribute("content") || "" : "";
    if (pathBase && pathBase.charAt(0) !== "/") pathBase = "/" + pathBase;
    if (pathBase.endsWith("/")) pathBase = pathBase.slice(0, -1);

    return window.location.origin + pathBase + rel;
  }

  /**
   * POST JSON med cookies. Kaster ikke — returnerer { ok, status, body, error }.
   */
  async function postJson(path, body, timeoutMs) {
    timeoutMs = timeoutMs || 120000;
    var url = apiUrl(path);

    var controller = typeof AbortController !== "undefined" ? new AbortController() : null;
    var timer = controller
      ? setTimeout(function () {
          controller.abort();
        }, timeoutMs)
      : null;

    try {
      var response = await fetch(url, {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json"
        },
        body: JSON.stringify(body),
        signal: controller ? controller.signal : undefined
      });

      var text = await response.text();
      return { ok: response.ok, status: response.status, body: text, error: null };
    } catch (err) {
      var msg = err && err.message ? err.message : "Netværksfejl";
      if (err && err.name === "AbortError") {
        msg = "Forespørgslen tog for lang tid (timeout efter " + Math.round(timeoutMs / 1000) + " sek). AI kan stadig arbejde — prøv igen om lidt.";
      }
      return { ok: false, status: 0, body: "", error: msg };
    } finally {
      if (timer) clearTimeout(timer);
    }
  }

  async function getJson(path, timeoutMs) {
    timeoutMs = timeoutMs || 30000;
    var url = apiUrl(path);
    var controller = typeof AbortController !== "undefined" ? new AbortController() : null;
    var timer = controller
      ? setTimeout(function () {
          controller.abort();
        }, timeoutMs)
      : null;

    try {
      var response = await fetch(url, {
        credentials: "include",
        headers: { Accept: "application/json" },
        signal: controller ? controller.signal : undefined
      });
      var text = await response.text();
      return { ok: response.ok, status: response.status, body: text, error: null };
    } catch (err) {
      var msg = err && err.message ? err.message : "Netværksfejl";
      if (err && err.name === "AbortError") {
        msg = "Forespørgslen tog for lang tid.";
      }
      return { ok: false, status: 0, body: "", error: msg };
    } finally {
      if (timer) clearTimeout(timer);
    }
  }

  global.gf2Api = {
    url: apiUrl,
    postJson: postJson,
    getJson: getJson
  };
})(window);
