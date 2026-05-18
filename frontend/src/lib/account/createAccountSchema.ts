// src/lib/account/createAccountSchema.ts

import { z } from 'zod';

/**
 * Create Account Schema
 *
 * This is the single source of truth for all Create Account validation.
 * Client-side validation, server-side validation, and integration tests
 * should all rely on this schema.
 */
export const CreateAccountSchema = z
  .object({
    email: z
      .string()
      .trim()
      .min(1, { message: 'Email is required' })
      .email({ message: 'Invalid email address' }),

    password: z
      .string()
      .min(1, { message: 'Password is required' })
      .min(8, { message: 'Password must be at least 8 characters' }),

    confirmPassword: z
      .string()
      .min(1, { message: 'Confirm password is required' }),
  })
  .superRefine((data, ctx) => {
    if (data.password !== data.confirmPassword) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ['confirmPassword'],
        message: 'Passwords do not match',
      });
    }
  });

export type CreateAccountValues = z.infer<typeof CreateAccountSchema>;
