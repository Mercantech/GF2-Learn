(function (global) {
  function apiUrl(path) {
    var base = document.querySelector("base");
    var href = base ? base.getAttribute("href") || "/" : "/";
    if (!href.endsWith("/")) href += "/";
    return href + path.replace(/^\//, "");
  }

  /**
   * POST med browser-cookies. Returnerer JSON-streng så Blazor kan parse body pålideligt.
   */
  async function postJson(path, body) {
    var response = await fetch(apiUrl(path), {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json"
      },
      body: JSON.stringify(body)
    });

    var text = await response.text();
    return JSON.stringify({
      ok: response.ok,
      status: response.status,
      body: text
    });
  }

  async function getStatus() {
    var response = await fetch(apiUrl("/api/exercise-ai/status"), {
      credentials: "include",
      headers: { Accept: "application/json" }
    });
    if (!response.ok) return JSON.stringify({ enabled: false, message: "HTTP " + response.status });
    var text = await response.text();
    return text;
  }

  global.gf2ExerciseAi = {
    getStatus: getStatus,
    postHint: function (body) {
      return postJson("/api/exercise-ai/hint", body);
    },
    postCheck: function (body) {
      return postJson("/api/exercise-ai/check", body);
    }
  };
})(window);
