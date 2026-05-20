export interface FormCommand<T> {
  run: (values: T) => Promise<void>;
  errors?: Record<string, string>;
  error?: string;
  isSubmitting: boolean;
}
