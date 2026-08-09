# Frontend e2e / screenshot tests (Playwright)

The scaffold ships with a small Playwright suite that runs the app off **mocked
API responses** (no backend required) and captures full-page **screenshots** for
visual review. This is the Northstar standard for frontend e2e — copy the shape
into new features and add routes to `mocks.ts` as the app grows.

## Layout

| File | Purpose |
|---|---|
| `../playwright.config.ts` | Config — boots the Vite dev server, targets Chromium. |
| `mocks.ts` | One place that fulfils every API call the app makes. Add a route here when the frontend starts calling a new endpoint. |
| `home.spec.ts` | Smoke asserts + screenshots for the home page (light + dark). |
| `screenshots/` | Generated PNGs land here. |

## Run locally

```bash
cd frontend
npm install
npx playwright install chromium   # one-time: downloads the browser
npm run test:e2e                  # headless; starts the dev server for you
npm run test:e2e -- --ui          # interactive runner
```

Screenshots are written to `frontend/e2e/screenshots/`; `npx playwright
show-report` opens the HTML report after a run.

## Gotchas worth knowing before you copy this

**1. `baseURL` must carry Vite's `base`.** This scaffold sets
`base: '/your-app-name/'` in `vite.config.ts`. The dev server redirects a bare
`/` to that base, so the home page works either way — but any deeper route
(`/editor`, `/settings`) served from a `baseURL` without the base lands on
Vite's *"the server is configured with a public base URL"* hint page, and your
assertions then run against the wrong document. Keep `baseURL` in
`playwright.config.ts` in sync with `base`, and navigate with **relative**
paths (`page.goto('./settings')`) — a leading slash discards the base again.

**2. Screenshot after a theme toggle needs `animations: 'disabled'`.** The
theme change is a 0.4s CSS transition, so a capture taken straight after the
click freezes a half-faded frame and dark mode looks broken when it isn't.
`animations: 'disabled'` fast-forwards finite transitions to completion.

**3. If the app also has vitest**, exclude `e2e/**` from it — otherwise vitest
collects the Playwright specs and `npm run test` fails with *"Playwright Test
did not expect test.beforeEach() to be called here"*.

## CI (recommended)

Add this workflow so every PR runs the tests and attaches the screenshots as a
downloadable artifact. GitHub runners ship all the browser deps, so
`--with-deps` just works there.

```yaml
# .github/workflows/frontend-e2e.yml
name: frontend-e2e
on:
  pull_request:
    paths: ['frontend/**']
jobs:
  e2e:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: 20
      - run: npm ci
      - run: npx playwright install --with-deps chromium
      - run: npm run test:e2e
      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: e2e-screenshots
          path: frontend/e2e/screenshots/
```

Download the `e2e-screenshots` artifact from the PR's checks to review.
