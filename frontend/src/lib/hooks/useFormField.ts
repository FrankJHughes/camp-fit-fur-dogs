export function useFormField(name: string, error?: string) {
  const id = `field-${name}`;
  const errorId = `error-${name}`;

  return {
    id,
    labelProps: { htmlFor: id },
    fieldProps: {
      id,
      'aria-invalid': error ? true : undefined,
      'aria-describedby': error ? errorId : undefined,
    },
    errorId,
  };
}
