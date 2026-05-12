import type { ReactNode } from 'react';
import { useFormField } from '../hooks/useFormField';
import { FieldError } from './FieldError';

export interface FieldProps {
  'aria-invalid'?: boolean;
  'aria-describedby'?: string;
}

export interface FormFieldProps {
  label: string;
  name: string;
  error?: string;
  children: (fieldProps: FieldProps) => ReactNode;
}

export function FormField({ label, name, error, children }: FormFieldProps) {
  const { labelProps, fieldProps, errorId } = useFormField(name, error);

  return (
    <div>
      <label {...labelProps}>{label}</label>
      {children(fieldProps)}
      <FieldError id={errorId} message={error} />
    </div>
  );
}
