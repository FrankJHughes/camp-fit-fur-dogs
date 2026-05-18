import { DogFormSchema, DogFormValues } from './DogFormSchema';

export function validateDogForm(values: DogFormValues): Record<string, string> {
  const result = DogFormSchema.safeParse(values);

  if (result.success) return {};

  const flat = result.error.flatten().fieldErrors;

  const errors: Record<string, string> = {};

  for (const key in flat) {
    const typedKey = key as keyof typeof flat;
    const msg = flat[typedKey]?.[0];
    if (msg) errors[key] = msg;
  }

  return errors;
}
