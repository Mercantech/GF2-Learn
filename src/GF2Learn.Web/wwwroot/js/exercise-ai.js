(function (global) {
  function wrap(envelope) {
    return JSON.stringify(envelope);
  }

  async function getStatus() {
    var result = await global.gf2Api.getJson("/api/exercise-ai/status");
    if (!result.ok) {
      return JSON.stringify({ enabled: false, message: result.error || "HTTP " + result.status });
    }
    return result.body;
  }

  async function postHint(body) {
    var result = await global.gf2Api.postJson("/api/exercise-ai/hint", body, 120000);
    return wrap(result);
  }

  async function postCheck(body) {
    var result = await global.gf2Api.postJson("/api/exercise-ai/check", body, 120000);
    return wrap(result);
  }

  global.gf2ExerciseAi = {
    getStatus: getStatus,
    postHint: postHint,
    postCheck: postCheck
  };
})(window);
