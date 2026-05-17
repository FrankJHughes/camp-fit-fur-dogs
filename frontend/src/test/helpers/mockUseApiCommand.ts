// test/helpers/mockUseApiCommand.ts
import { vi } from 'vitest';
import type { MockInstance } from 'vitest';

export const useApiCommandMock = vi.fn();

vi.mock('@/lib/hooks/useApiCommand', () => ({
  useApiCommand: useApiCommandMock,
}));

export function setUseApiCommandReturn(value: any) {
  (useApiCommandMock as unknown as MockInstance).mockReturnValue(value);
}

export function resetUseApiCommandMock() {
  useApiCommandMock.mockReset();
}
