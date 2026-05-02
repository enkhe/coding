/// <reference types="vite/client" />

// Type the env vars exposed to the client (must be prefixed with VITE_)
interface ImportMetaEnv {
  readonly VITE_API_BASE: string;
  readonly VITE_FEATURE_FLAGS?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
