import { useCallback, useEffect, useRef, useState } from 'react';

type FormState<T> = {
  values: T;
  errors: Record<string, string>;
  isSubmitting: boolean;
};

type OnSubmitResult = Record<string, string> | void | Promise<Record<string, string> | void>;

export function useFormStateMachine<T extends Record<string, any>>(opts: {
  initialValues: T;
  externalErrors?: Record<string, string> | undefined;
  onSubmit?: (values: T) => OnSubmitResult;
}) {
  const { initialValues, externalErrors, onSubmit } = opts;

  const [state, setState] = useState<FormState<T>>({
    values: initialValues,
    errors: externalErrors ?? {},
    isSubmitting: false,
  });

  const valuesRef = useRef<T>(initialValues);
  valuesRef.current = state.values;

  const previousExternalErrors = useRef<string | undefined>(
    externalErrors ? JSON.stringify(externalErrors) : undefined
  );

  const mountedRef = useRef(true);
  useEffect(() => {
    return () => {
      mountedRef.current = false;
    };
  }, []);

  useEffect(() => {
    const currentSerialized = externalErrors ? JSON.stringify(externalErrors) : undefined;
    if (currentSerialized === previousExternalErrors.current) {
      return;
    }
    previousExternalErrors.current = currentSerialized;
    setState((prev) => ({ ...prev, errors: externalErrors ?? prev.errors }));
  }, [externalErrors]);

  useEffect(() => {
    const prev = state.values;
    const changed =
      Object.keys(initialValues).length !== Object.keys(prev).length ||
      Object.keys(initialValues).some((k) => initialValues[k] !== prev[k]);

    if (changed) {
      setState((prev) => ({ ...prev, values: initialValues }));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialValues]);

  const update = useCallback(
    (field: keyof T) =>
      (eOrValue: any) => {
        const value =
          eOrValue && typeof eOrValue === 'object' && 'target' in eOrValue
            ? (eOrValue.target as HTMLInputElement).value
            : eOrValue;

        setState((prev) => {
          const nextValues = { ...prev.values, [field]: value };
          valuesRef.current = nextValues;
          return { ...prev, values: nextValues };
        });
      },
    []
  );

  const handleSubmit = useCallback(
    async (e?: React.FormEvent) => {
      e?.preventDefault();
      if (!onSubmit) return;

      setState((prev) => ({ ...prev, isSubmitting: true }));

      try {
        const result = await onSubmit(valuesRef.current);

        if (result && Object.keys(result).length > 0) {
          if (mountedRef.current) {
            setState((prev) => ({ ...prev, errors: result }));
          }
          return;
        }

        if (mountedRef.current) {
          setState((prev) => ({ ...prev, errors: {} }));
        }
      } finally {
        if (mountedRef.current) {
          setState((prev) => ({ ...prev, isSubmitting: false }));
        }
      }
    },
    [onSubmit]
  );

  return {
    state,
    values: state.values,
    displayErrors: state.errors ?? {},
    update,
    handleSubmit,
    isSubmitting: state.isSubmitting,
  } as const;
}
