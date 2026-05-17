"use client";

import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";

const CreateAccountSchema = z
  .object({
    email: z
      .string()
      .min(1, "Email is required")
      .regex(/^[^\s@]+@[^\s@]+\.[^\s@]+$/, "Invalid email"),

    password: z
      .string()
      .min(1, "Password is required")
      .min(8, "Password must be at least 8 characters"),

    confirmPassword: z.string().min(1, "Confirm password is required"),
  })
  .superRefine(({ password, confirmPassword }, ctx) => {
    if (password !== confirmPassword) {
      ctx.addIssue({
        code: "custom",
        message: "Passwords do not match",
        path: ["confirmPassword"],
      });
    }
  });

type CreateAccountForm = z.infer<typeof CreateAccountSchema>;

export default function CreateAccountPage({
  onSubmit,
}: {
  onSubmit?: (data: CreateAccountForm) => Promise<void> | void;
}) {
  const router = useRouter();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    watch,
  } = useForm<CreateAccountForm>({
    resolver: zodResolver(CreateAccountSchema),
  });

  const [success, setSuccess] = useState(false);

  // Watch individual fields
  const email = watch("email");
  const password = watch("password");
  const confirmPassword = watch("confirmPassword");

  // Track previous field values
  const prev = useRef({ email, password, confirmPassword });

  useEffect(() => {
    const userEdited =
      email !== prev.current.email ||
      password !== prev.current.password ||
      confirmPassword !== prev.current.confirmPassword;

    // Only clear success if the user edited AFTER success was set
    if (success && userEdited) {
      setSuccess(false);
    }

    prev.current = { email, password, confirmPassword };
  }, [email, password, confirmPassword, success]);

  const submitHandler = handleSubmit(async (data) => {
    if (isSubmitting) return;

    if (onSubmit) {
      await onSubmit(data);
    }

    setSuccess(true);

    router.push("/owners/create-account/success");
  });

  return (
    <form onSubmit={submitHandler}>
      <div>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          {...register("email")}
          type="text"
          name="email"
          disabled={isSubmitting}
        />
        {errors.email && <p>{errors.email.message}</p>}
      </div>

      <div>
        <label htmlFor="password">Password</label>
        <input
          id="password"
          {...register("password")}
          type="password"
          name="password"
          disabled={isSubmitting}
        />
        {errors.password && <p>{errors.password.message}</p>}
      </div>

      <div>
        <label htmlFor="confirmPassword">Confirm Password</label>
        <input
          id="confirmPassword"
          {...register("confirmPassword")}
          type="password"
          name="confirmPassword"
          disabled={isSubmitting}
        />
        {errors.confirmPassword && <p>{errors.confirmPassword.message}</p>}
      </div>

      <button type="submit" disabled={isSubmitting}>
        Create Account
      </button>

      {success && <p>Account created successfully</p>}
    </form>
  );
}
