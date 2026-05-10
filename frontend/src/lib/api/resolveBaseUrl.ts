// src/lib/resolveBaseUrl.ts
export function resolveBaseUrl() {
  const envBase = process.env.NEXT_PUBLIC_API_BASE_URL;
  const localBase = "http://localhost:5209";

  if (envBase && envBase.trim().length > 0) {
    return envBase; // Vercel preview + production
  }

  if (process.env.NODE_ENV === "development" || process.env.NODE_ENV === "test") {
    return localBase; // Local dev
  }

  throw new Error(
    "NEXT_PUBLIC_API_BASE_URL is not set in production environment."
  );
}
