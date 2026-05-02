// Playwright E2E — accessible queries, network interception, visual snapshot.
// npx playwright install
import { test, expect } from '@playwright/test';

test.describe('orders flow', () => {
  test.beforeEach(async ({ page }) => {
    // Stub the API so we don't depend on a real backend.
    await page.route('**/api/orders', async (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([{ id: 'o-1', total: 9.99 }]),
      });
    });
  });

  test('shows orders list', async ({ page }) => {
    await page.goto('/orders');
    await expect(page.getByRole('heading', { name: /orders/i })).toBeVisible();
    await expect(page.getByText('o-1')).toBeVisible();
  });

  test('places an order', async ({ page }) => {
    await page.goto('/orders');
    await page.getByRole('button', { name: /place order/i }).click();
    await expect(page.getByText(/order placed/i)).toBeVisible();
  });

  test('visual: header looks the same', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('header')).toHaveScreenshot('header.png');
  });
});
