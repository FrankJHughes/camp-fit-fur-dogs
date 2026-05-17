// @vitest-environment jsdom
import { renderHook, waitFor } from '@testing-library/react';
import { useApiQuery, type QueryState } from '@/lib/hooks/useApiQuery';

describe('useApiQuery', () => {
  it('starts in loading state', () => {
    const { result } = renderHook(() =>
      useApiQuery(() => new Promise<QueryState<string>>(() => {}), [])
    );
    expect(result.current).toEqual({ status: 'loading' });
  });

  it('transitions to success state', async () => {
    const { result } = renderHook(() =>
      useApiQuery(
        () => Promise.resolve({ status: 'success' as const, data: 'hello' }),
        []
      )
    );
    await waitFor(() => {
      expect(result.current).toEqual({ status: 'success', data: 'hello' });
    });
  });

  it('transitions to not-found state', async () => {
    const { result } = renderHook(() =>
      useApiQuery(
        () => Promise.resolve({ status: 'not-found' as const }),
        []
      )
    );
    await waitFor(() => {
      expect(result.current).toEqual({ status: 'not-found' });
    });
  });

  it('transitions to error state', async () => {
    const { result } = renderHook(() =>
      useApiQuery(
        () => Promise.resolve({ status: 'error' as const, error: 'boom' }),
        []
      )
    );
    await waitFor(() => {
      expect(result.current).toEqual({ status: 'error', error: 'boom' });
    });
  });

  it('resets to loading when deps change', async () => {
    let resolveQuery: (value: QueryState<string>) => void;
    const queryFn = vi.fn(() => new Promise<QueryState<string>>((resolve) => {
      resolveQuery = resolve;
    }));

    const { result, rerender } = renderHook(
      ({ dep }) => useApiQuery(queryFn, [dep]),
      { initialProps: { dep: 1 } }
    );

    resolveQuery!({ status: 'success', data: 'first' });
    await waitFor(() => {
      expect(result.current).toEqual({ status: 'success', data: 'first' });
    });

    rerender({ dep: 2 });
    expect(result.current).toEqual({ status: 'loading' });
  });
});