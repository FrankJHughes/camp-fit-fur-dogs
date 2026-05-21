export type FormCommand<T> = {
  run: (payload: T) => Promise<void> | void;
  errors?: Record<string, string> | undefined;
  error?: string | undefined;
  isSubmitting?: boolean;
};
