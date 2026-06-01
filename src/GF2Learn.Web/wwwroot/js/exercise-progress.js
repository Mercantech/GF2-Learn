(function (global) {
  function apiUrl(path) {
    var base = document.querySelector("base");
    var href = base ? base.getAttribute("href") || "/" : "/";
    if (!href.endsWith("/")) href += "/";
    return href + path.replace(/^\//, "");
  }

  function normalizePart(raw) {
    return {
      partIndex: raw.partIndex ?? raw.PartIndex,
      answerText: raw.answerText ?? raw.AnswerText ?? "",
      completedAt: raw.completedAt ?? raw.CompletedAt
    };
  }

  function readBootstrapAnswers(contentSlug) {
    var el = document.getElementById("exercise-bootstrap-" + contentSlug);
    if (!el || !el.textContent) return null;

    try {
      var parsed = JSON.parse(el.textContent);
      return Array.isArray(parsed) ? parsed : null;
    } catch (e) {
      console.warn("Exercise progress: kunne ikke læse gemte svar.", e);
      return null;
    }
  }

  function markExerciseCompleteInNav(contentSlug) {
    if (!contentSlug) return;

    document.querySelectorAll('[data-exercise-slug="' + contentSlug + '"]').forEach(function (link) {
      if (link.querySelector(".sidebar-check, .index-check")) return;

      if (link.classList.contains("sidebar-link")) {
        var check = document.createElement("span");
        check.className = "sidebar-check";
        check.setAttribute("aria-label", "Fuldført");
        check.setAttribute("title", "Alle opgavedele løst");
        check.textContent = "✓";
        link.appendChild(check);
        return;
      }

      if (link.classList.contains("index-item")) {
        var arrow = link.querySelector(".index-arrow");
        if (arrow) arrow.remove();
        var indexCheck = document.createElement("span");
        indexCheck.className = "index-check";
        indexCheck.setAttribute("aria-label", "Fuldført");
        indexCheck.setAttribute("title", "Alle opgavedele løst");
        indexCheck.textContent = "✓";
        link.appendChild(indexCheck);
      }
    });
  }

  function updateProgressSummary(root) {
    var summary = root.querySelector(".exercise-progress-summary");
    if (!summary) return;

    var total = root.querySelectorAll(".exercise-part").length;
    var done = root.querySelectorAll(".exercise-part.exercise-part-complete").length;
    if (total === 0) {
      summary.hidden = true;
      return;
    }

    summary.textContent = done + " af " + total + " opgavedele gemt som løst";
    summary.hidden = false;

    if (done === total) {
      markExerciseCompleteInNav(root.dataset.contentSlug);
    }
  }

  function applySavedPart(part, answerText) {
    part.classList.add("exercise-part-complete");
    var textarea = part.querySelector(".exercise-answer-input");
    var badge = part.querySelector(".exercise-saved-badge");
    if (textarea && answerText) textarea.value = answerText;
    if (badge) badge.hidden = false;
  }

  function savePart(contentSlug, partIndex, answerText) {
    return fetch(apiUrl("/api/progress/exercise"), {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        contentSlug: contentSlug,
        partIndex: partIndex,
        answerText: answerText
      })
    }).then(function (response) {
      return response.ok;
    }).catch(function () {
      return false;
    });
  }

  function markPartComplete(contentSlug, partIndex) {
    var root = document.querySelector('.exercise-page[data-content-slug="' + contentSlug + '"]');
    if (!root) return;

    var part = root.querySelector('.exercise-part[data-part-index="' + partIndex + '"]');
    if (part) part.classList.add("exercise-part-complete");

    updateProgressSummary(root);
  }

  function loadAnswers(contentSlug) {
    return fetch(apiUrl("/api/progress/exercise/" + encodeURIComponent(contentSlug)), {
      credentials: "include",
      headers: { Accept: "application/json" }
    }).then(function (response) {
      if (response.status === 401) return [];
      if (!response.ok) return [];
      var contentType = response.headers.get("content-type") || "";
      if (contentType.indexOf("application/json") === -1) return [];
      return response.json().then(function (data) {
        return Array.isArray(data) ? data : [];
      });
    }).catch(function () {
      return [];
    });
  }

  function showLoginHint(root) {
    if (root.dataset.exerciseLoginHintShown === "1") return;
    root.dataset.exerciseLoginHintShown = "1";
    var hint = document.createElement("p");
    hint.className = "exercise-login-hint";
    hint.innerHTML = '<a href="/auth/login">Log ind</a> for at gemme dine løsninger på tværs af enheder.';
    var summary = root.querySelector(".exercise-progress-summary");
    if (summary) summary.after(hint);
    else root.prepend(hint);
  }

  function wirePart(part, root) {
    if (part.dataset.exerciseWired === "1") return;
    part.dataset.exerciseWired = "1";

    var contentSlug = root.dataset.contentSlug || part.dataset.contentSlug;
    var partIndex = parseInt(part.dataset.partIndex, 10);
    var btn = part.querySelector(".exercise-save-btn");
    var textarea = part.querySelector(".exercise-answer-input");
    if (!btn || !textarea) return;

    btn.addEventListener("click", function () {
      var answerText = textarea.value.trim();
      if (!answerText) {
        textarea.focus();
        return;
      }

      applySavedPart(part, answerText);
      updateProgressSummary(root);

      if (!contentSlug) return;

      savePart(contentSlug, partIndex, answerText).then(function (response) {
        if (response.ok) {
          updateProgressSummary(root);
          return;
        }
        if (response.status === 401) showLoginHint(root);
      });
    });
  }

  function restoreParts(root, answers) {
    if (!answers || !answers.length) return;

    answers.forEach(function (raw) {
      var part = normalizePart(raw);
      if (part.partIndex === undefined) return;
      var el = root.querySelector('.exercise-part[data-part-index="' + part.partIndex + '"]');
      if (!el) return;
      applySavedPart(el, part.answerText || "");
    });

    updateProgressSummary(root);
  }

  function initExercisePage(root) {
    var contentSlug = root.dataset.contentSlug;
    if (!contentSlug) return;

    root.querySelectorAll(".exercise-part").forEach(function (part) {
      wirePart(part, root);
    });

    function finishInit(answers) {
      restoreParts(root, answers);
    }

    var bootstrap = readBootstrapAnswers(contentSlug);
    if (bootstrap) {
      finishInit(bootstrap);
      return;
    }

    loadAnswers(contentSlug).then(finishInit);
  }

  function initExerciseProgress(scope) {
    scope = scope || document;
    scope.querySelectorAll(".exercise-page[data-content-slug]").forEach(initExercisePage);
  }

  function scheduleInit(scope) {
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        initExerciseProgress(scope);
      });
    });
  }

  function wire() {
    scheduleInit(document);
    global.addEventListener("gf2-enhanced-nav", function () {
      scheduleInit(document);
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", wire);
  } else {
    wire();
  }

  global.gf2ExerciseProgress = {
    init: initExerciseProgress,
    savePart: savePart,
    markPartComplete: markPartComplete
  };
})(window);
