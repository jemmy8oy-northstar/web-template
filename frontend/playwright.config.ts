import { defineConfig, devices } from '@playwright/test';

/**
 * Playwright e2e config for the frontend.
 *
 * These tests drive the app off mocked API responses (see e2e/mocks.ts), so
 * they need no backend and render deterministically. Each spec also captures
 * full-page screenshots into e2e/screenshots/ for visual review.
 *
 * Run:  npm run test:e2e            (headless, starts the dev server for you)
 *       npm run test:e2e -- --ui    (interactive)
 *
 * Note: the CI job (see e2e/README.md) uploads e2e/screenshots/ as a build
 * artifact so screenshots are viewable per-PR.
 */
export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: process.env.CI ? [['html', { open: 'never' }], ['list']] : 'list',
  use: {
    baseURL: 'http://localhost:4173',
    trace: 'on-first-retry',
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
  ],
  webServer: {
    command: 'npm run dev -- --port 4173 --strictPort',
    url: 'http://localhost:4173',
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
  },
});
