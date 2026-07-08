import { resolveBaseUrl } from "./resolveBaseUrl";

export interface ApiError {
  type: 'network' | 'http' | 'validation';
  message: string;
  status?: number;
  errors?: Record<string, string[]>;
}

export type ApiResult<T> =
  | { ok: true; data: T }
  | { ok: false; error: ApiError };

export function createApiClient(baseUrl: string = resolveBaseUrl()) {
  async function request<T>(
    method: string,
    path: string,
    body?: unknown,
  ): Promise<ApiResult<T>> {
    try {
      const options: RequestInit = {
        method,
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include', // important for session cookies
      };

      if (body !== undefined) {
        options.body = JSON.stringify(body);
      }

      const response = await fetch(`${baseUrl}${path}`, options);

      // ❗ Do NOT parse JSON yet — check status first
      if (!response.ok) {
        // 422 → validation errors (JSON body expected)
        if (response.status === 422) {
          const data = await safeJson(response);
          return {
            ok: false,
            error: {
              type: 'validation',
              status: 422,
              message: data?.message ?? 'Validation failed',
              errors: data?.errors,
            },
          };
        }

        // 401/404/500 → may have NO body
        return {
          ok: false,
          error: {
            type: 'http',
            status: response.status,
            message: response.statusText || 'Request failed',
          },
        };
      }

      // ✔ Safe to parse JSON now
      const data = await safeJson(response);

      return { ok: true, data: data as T };

    } catch (error) {
      return {
        ok: false,
        error: {
          type: 'network',
          message: error instanceof Error ? error.message : 'Unknown error',
        },
      };
    }
  }

  // Helper: safely parse JSON or return null
  async function safeJson(res: Response): Promise<any | null> {
    try {
      return await res.json();
    } catch {
      return null;
    }
  }

  return {
    get: <T>(path: string) => request<T>('GET', path),
    post: <T>(path: string, body: unknown) => request<T>('POST', path, body),
    put: <T>(path: string, body: unknown) => request<T>('PUT', path, body),
    delete: <T>(path: string) => request<T>('DELETE', path),
  };
}
