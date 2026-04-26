import { FieldError } from './FieldError';

export interface FieldProps {
  'aria-invalid'?: true;
  'aria-describedby'?: string;
}

interface FormFieldProps {
  label: string;
  name: string;
  error: string | undefined;
  children: (fieldProps: FieldProps) => React.ReactNode;
}

export function FormField({ label, name, error, children }: FormFieldProps) {
  const errorId = `error-${name}`;

  const fieldProps: FieldProps = {
    'aria-invalid': error ? true : undefined,
    'aria-describedby': error ? errorId : undefined,
  };

  return (
    <>
      <label>
        {label}
        {children(fieldProps)}
      </label>
      <FieldError id={errorId} message={error} />
    </>
  );
}