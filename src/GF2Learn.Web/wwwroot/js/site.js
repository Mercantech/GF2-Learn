(function () {
  function readMeta(name) {
    var element = document.querySelector('meta[name="' + name + '"]');
    return element ? element.getAttribute("content") || "" : "";
  }

  function readUnixTime(name) {
    var value = Number.parseInt(readMeta(name), 10);
    return Number.isFinite(value) ? value : null;
  }

  function formatDate(unixSeconds) {
    if (unixSeconds === null) return "Ikke tilgængelig";
    return new Date(unixSeconds * 1000).toLocaleString("da-DK");
  }

  function formatDuration(seconds) {
    if (!Number.isFinite(seconds)) return "Ikke tilgængelig";

    var sign = seconds < 0 ? "-" : "";
    var remaining = Math.abs(Math.round(seconds));
    var days = Math.floor(remaining / 86400);
    remaining %= 86400;
    var hours = Math.floor(remaining / 3600);
    remaining %= 3600;
    var minutes = Math.floor(remaining / 60);
    var secs = remaining % 60;
    var parts = [];

    if (days) parts.push(days + " d");
    if (hours || days) parts.push(hours + " t");
    if (minutes || hours || days) parts.push(minutes + " min");
    parts.push(secs + " sek");
    return sign + parts.join(" ");
  }

  function sessionRow(issuedAt, expiresAt, now) {
    return {
      "Udstedt": formatDate(issuedAt),
      "Udløber": formatDate(expiresAt),
      "Samlet levetid": issuedAt !== null && expiresAt !== null
        ? formatDuration(expiresAt - issuedAt)
        : "Ikke tilgængelig",
      "Resterende": expiresAt !== null
        ? formatDuration(expiresAt - now)
        : "Ikke tilgængelig"
    };
  }

  function logAuthSession() {
    if (readMeta("gf2-authenticated") !== "true") {
      console.info("[GF2 Auth] Brugeren er ikke logget ind.");
      return;
    }

    var accessIssuedAt = readUnixTime("gf2-auth-access-issued-at");
    var accessExpiresAt = readUnixTime("gf2-auth-access-expires-at");
    var cookieIssuedAt = readUnixTime("gf2-auth-cookie-issued-at");
    var cookieExpiresAt = readUnixTime("gf2-auth-cookie-expires-at");
    var now = Date.now() / 1000;

    console.group("[GF2 Auth] Sessionens tokenlevetider");
    console.table({
      "Access-token": sessionRow(accessIssuedAt, accessExpiresAt, now),
      "GF2 login-cookie": sessionRow(cookieIssuedAt, cookieExpiresAt, now)
    });
    console.info("Refresh-tokenfornyelse i GF2 Learn: deaktiveret");
    console.info("Sliding expiration for GF2-cookie: deaktiveret");

    if (accessExpiresAt === null || cookieExpiresAt === null) {
      console.info("[GF2 Auth] Tidsmetadata mangler på en ældre session. Log ud og ind igen efter deployment for at få de fulde værdier.");
    }

    console.groupEnd();
  }

  window.gf2AuthDiagnostics = { log: logAuthSession };

  function enhancePlaygrounds() {
    document.querySelectorAll(".code-playground:not(.code-playground-interactive)").forEach(function (el) {
      if (el.dataset.enhanced) return;
      el.dataset.enhanced = "1";
      var code = el.getAttribute("data-code") || "";
      var expected = el.getAttribute("data-expected") || "";
      var pre = el.querySelector(".playground-code");
      if (pre) pre.textContent = code;

      var toolbar = document.createElement("div");
      toolbar.className = "playground-toolbar";

      var copyBtn = document.createElement("button");
      copyBtn.type = "button";
      copyBtn.textContent = "Kopier kode";
      copyBtn.addEventListener("click", function () {
        navigator.clipboard.writeText(code).then(function () {
          copyBtn.textContent = "Kopieret!";
          setTimeout(function () { copyBtn.textContent = "Kopier kode"; }, 1500);
        });
      });

      toolbar.appendChild(copyBtn);
      el.appendChild(toolbar);

      if (expected) {
        var out = document.createElement("div");
        out.className = "playground-expected";
        out.textContent = "Forventet output: " + expected;
        el.appendChild(out);
      }
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    enhancePlaygrounds();
    logAuthSession();
  });
  document.addEventListener("gf2-enhanced-nav", enhancePlaygrounds);

  document.addEventListener("DOMContentLoaded", function () {
    if (window.gf2Highlight) window.gf2Highlight.process(document);
  });
})();

window.gf2ScrollToBottom = function (element) {
  if (!element) return;
  requestAnimationFrame(function () {
    element.scrollTop = element.scrollHeight;
  });
};
