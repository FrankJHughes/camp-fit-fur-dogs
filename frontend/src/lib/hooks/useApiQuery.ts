import { useEffect, useState } from 'react';

export type QueryState<T> =
  | { status: 'loading' }
  | { status: 'success'; data: T }
  | { status: 'not-found' }
  | { status: 'error'; error: string };

export function useApiQuery<T>(
  queryFn: () => Promise<QueryState<T>>,
  deps: unknown[]
): QueryState<T> {
  const [state, setState] = useState<QueryState<T>>({ status: 'loading' });

  useEffect(() => {
    setState({ status: 'loading' });
    queryFn().then(setState);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps);

  return state;
}