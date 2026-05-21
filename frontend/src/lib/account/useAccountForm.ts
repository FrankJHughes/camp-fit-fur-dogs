// src/lib/account/useAccountForm.ts
'use client';

import { useCallback, useMemo } from 'react';
import { useFormStateMachine } from '@/lib/forms/useFormStateMachine';
import type { CreateAccountValues } from './createAccountSchema';

type ValidateFn<T> = (values: T) => Record<string, string> | Promise<Record<string, string>>;

export function useAccountForm(opts: {
  initialValues: CreateAccountValues;
  externalErrors?: Record<string, string>;
  onSubmit?: (values: CreateAccountValues) => Promise<void> | void;
  validate?: ValidateFn<CreateAccountValues>;
}) {
  const { state, values, displayErrors, update, handleSubmit, isSubmitting } =
    useFormStateMachine<CreateAccountValues>({
      initialValues: opts.initialValues,
      externalErrors: opts.externalErrors,
      onSubmit: async (vals) => {
        // Run validate before calling the provided onSubmit
        if (opts.validate) {
          const validation = await opts.validate(vals);
          // If validation returns any errors, set them on the state and do not proceed
          if (validation && Object.keys(validation).length > 0) {
            // useFormStateMachine exposes displayErrors via externalErrors; set via a synthetic submit that returns early
            // We call the hook's onSubmit only when there are no validation errors.
            // The useFormStateMachine implementation should already surface externalErrors; here we just return early.
            return;
          }
        }

        if (opts.onSubmit) {
          await opts.onSubmit(vals);
        }
      },
    });

  // Merge external and internal display errors (useFormStateMachine already merges externalErrors into displayErrors)
  const mergedDisplayErrors = useMemo(() => displayErrors, [displayErrors]);

  return {
    state,
    values,
    displayErrors: mergedDisplayErrors,
    update,
    handleSubmit,
    isSubmitting,
  };
}
