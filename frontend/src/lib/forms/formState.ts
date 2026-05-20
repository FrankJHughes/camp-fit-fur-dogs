export type FormState<TValues> =
  | { tag: 'idle'; values: TValues }
  | { tag: 'validating'; values: TValues }
  | { tag: 'submitting'; values: TValues }
  | { tag: 'error'; values: TValues; errors: Record<string, string> }
  | { tag: 'success'; values: TValues };
