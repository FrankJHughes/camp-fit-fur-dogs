export interface ApiError {
  type: 'network' | 'http' | 'validation';
  message: string;
  status?: number;
  errors?: Record<string, string[]>;
}

export type ApiResult<T> =
  | { ok: true; data: T }
  | { ok: false; error: ApiError };

export function createApiClient(baseUrl: string = '/api') {
  async function request<T>(
    method: string,
    path: string,
    body?: unknown,
  ): Promise<ApiResult<T>> {
    try {
      const options: RequestInit = {
        method,
        headers: { 'Content-Type': 'application/json' },
      };
      if (body !== undefined) {
        options.body = JSON.stringify(body);
      }
      const response = await fetch(`${baseUrl}${path}`, options);
      const data = await response.json();
      if (!response.ok) {
        if (response.status === 422) {
          return {
            ok: false,
            error: {
              type: 'validation',
              status: 422,
              message: data.message ?? 'Validation failed',
              errors: data.errors,
            },
          };
        }
        return {
          ok: false,
          error: {
            type: 'http',
            status: response.status,
            message: data.message ?? 'Request failed',
          },
        };
      }
      return { ok: true, data };
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

  return {
    get: <T>(path: string) => request<T>('GET', path),
    post: <T>(path: string, body: unknown) => request<T>('POST', path, body),
    put: <T>(path: string, body: unknown) => request<T>('PUT', path, body),
    delete: <T>(path: string) => request<T>('DELETE', path),
  };
}
