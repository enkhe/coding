import { defineConfig, devices } from '@playwright/test';

// JS/TS variant for front-end teams. Mirrors the .NET test scenarios.
export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  retries: process.env.CI ? 2 : 0,
  reporter: [['html', { open: 'never' }], ['list']],
  use: {
    baseURL: process.env.BASE_URL ?? 'https://app.local',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
    { name: 'webkit', use: { ...devices['Desktop Safari'] } },
    { name: 'mobile', use: { ...devices['iPhone 15'] } },
  ],
  webServer: {
    command: 'dotnet run --project ../../src/Web',
    url: 'https://localhost:5001/health/ready',
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
  },
});
