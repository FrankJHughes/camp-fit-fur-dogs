import { toQueryState } from '@/lib/api/queryResult';
import type { QueryResult } from '@/lib/api/queryResult';

describe('toQueryState', () => {
  it('maps success to success state with data', () => {
    const result: QueryResult<string> = { success: true, data: 'hello' };
    expect(toQueryState(result)).toEqual({ status: 'success', data: 'hello' });
  });

  it('maps notFound to not-found state', () => {
    const result: QueryResult<string> = { success: false, notFound: true };
    expect(toQueryState(result)).toEqual({ status: 'not-found' });
  });

  it('maps error to error state with message', () => {
    const result: QueryResult<string> = { success: false, notFound: false, error: 'Server error' };
    expect(toQueryState(result)).toEqual({ status: 'error', error: 'Server error' });
  });

  it('uses fallback message when error is undefined', () => {
    const result: QueryResult<string> = { success: false, notFound: false };
    expect(toQueryState(result)).toEqual({ status: 'error', error: 'An unknown error occurred.' });
  });
});