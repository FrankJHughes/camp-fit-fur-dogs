import { vi } from 'vitest';
import type { MockInstance } from 'vitest';

export const useApiQueryMock = vi.fn();

vi.mock('@/lib/hooks/useApiQuery', () => ({
  useApiQuery: useApiQueryMock,
}));

export function setUseApiQueryReturn(value: any) {
  (useApiQueryMock as unknown as MockInstance).mockReturnValue(value);
}

export function resetUseApiQueryMock() {
  useApiQueryMock.mockReset();
}
