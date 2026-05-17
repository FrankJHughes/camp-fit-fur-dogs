import type { ReactNode } from 'react';
import { useFormField } from '@/lib/hooks/useFormField';
import { FieldError } from '@/lib/components/FieldError';

export interface FieldProps {
  id: string;
  'aria-invalid'?: boolean;
  'aria-describedby'?: string;
  [key: string]: any;
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
