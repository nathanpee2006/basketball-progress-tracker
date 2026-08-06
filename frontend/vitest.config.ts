import { defineConfig } from 'vitest/config'
import { playwright } from '@vitest/browser-playwright'

export default defineConfig({
  test: {
    globals: true,
    browser: {
      provider: playwright(),
      enabled: true,
      instances: [
        { browser: 'chromium' },
      ],
    },
    restoreMocks: true,
  },
  resolve: {
    tsconfigPaths: true,
  },
})