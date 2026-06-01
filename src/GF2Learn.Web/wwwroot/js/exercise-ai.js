(function (global) {
  function apiUrl(path) {
    var base = document.querySelector("base");
    var href = base ? base.getAttribute("href") || "/" : "/";
    if (!href.endsWith("/")) href += "/";
    return href + path.replace(/^\//, "");
  }

  /**
   * POST med browser-cookies (Mercantec-login). WASM HttpClient sender ofte ikke cookies.
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
    var data = null;
    if (text) {
      try {
        data = JSON.parse(text);
      } catch (e) {
        data = null;
      }
    }

    return { ok: response.ok, status: response.status, data: data, text: text };
  }

  async function getStatus() {
    var response = await fetch(apiUrl("/api/exercise-ai/status"), {
      credentials: "include",
      headers: { Accept: "application/json" }
    });
    if (!response.ok) return { enabled: false, message: "HTTP " + response.status };
    return response.json();
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
