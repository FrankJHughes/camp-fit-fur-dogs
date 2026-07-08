import { z } from 'zod';

export const DogFormSchema = z.object({
  name: z.string().trim().min(1, "Please enter your dog's name"),
  breed: z.string().trim().min(1, 'Please enter a breed'),
  dateOfBirth: z.string().trim().min(1, 'Please enter a date of birth'),

  // Required enum with correct Zod error handling
  sex: z.enum(['Male', 'Female']).refine(
    (v) => v === 'Male' || v === 'Female',
    { message: 'Please select a sex' }
  ),
});

export type DogFormValues = z.infer<typeof DogFormSchema>;
