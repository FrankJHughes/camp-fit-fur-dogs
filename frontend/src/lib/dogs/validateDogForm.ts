import { z } from 'zod';

export const DogFormSchema = z.object({
  name: z.string().trim().min(1, "Please enter your dog's name"),
  breed: z.string().trim().min(1, 'Please enter a breed'),
  dateOfBirth: z.string().trim().min(1, 'Please enter a date of birth'),
  sex: z
    .string()
    .refine((v) => v === 'Male' || v === 'Female', {
      message: 'Please select a sex',
    }),
});

export type DogFormValues = z.infer<typeof DogFormSchema>;

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
