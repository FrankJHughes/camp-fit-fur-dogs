export interface FieldErrorProps {
  id: string;
  message: string | undefined;
}

export function FieldError({ id, message }: FieldErrorProps) {
  if (!message) return null;

  return (
    <p id={id} role="alert">
      {message}
    </p>
  );
}