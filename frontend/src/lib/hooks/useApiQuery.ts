import { useState, useEffect } from 'react';

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
  const [prevDeps, setPrevDeps] = useState(deps);

  if (!depsEqual(prevDeps, deps)) {
    setPrevDeps(deps);
    setState({ status: 'loading' });
  }

  useEffect(() => {
    queryFn().then(setState);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps);

  return state;
}

function depsEqual(a: unknown[], b: unknown[]): boolean {
  if (a.length !== b.length) return false;
  return a.every((val, i) => Object.is(val, b[i]));
}