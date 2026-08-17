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
  // Always prefix API routes with /api
  function buildUrl(path: string) {
    if (!path.startsWith("/"))
      throw new Error("API path must start with '/'");

    return `${baseUrl}/api${path}`;
  }

  async function request<T>(
    method: string,
    path: string,
    body?: unknown,
  ): Promise<ApiResult<T>> {
    try {
      const url = buildUrl(path);

      const options: RequestInit = {
        method,
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
      };

      if (body !== undefined) {
        options.body = JSON.stringify(body);
      }

      const response = await fetch(url, options);

      if (!response.ok) {
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

        return {
          ok: false,
          error: {
            type: 'http',
            status: response.status,
            message: response.statusText || 'Request failed',
          },
        };
      }

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
