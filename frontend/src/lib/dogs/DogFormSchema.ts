import { z } from 'zod';

export const DogFormSchema = z.object({
  name: z.string().trim().min(1, "Please enter your dog's name"),
  breed: z.string().trim().min(1, 'Please enter a breed'),
  dateOfBirth: z.string().trim().min(1, 'Please enter a date of birth'),
  sex: z
    .union([
      z.literal('Male'),
      z.literal('Female'),
      z.literal(''),
    ])
    .refine((v) => v === 'Male' || v === 'Female', {
      message: 'Please select a sex',
    }),
});

export type DogFormValues = z.infer<typeof DogFormSchema>;
