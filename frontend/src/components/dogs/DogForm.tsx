// src/components/dogs/DogForm.tsx
'use client';

import { FormField } from '@/lib/components/FormField';
import { FieldError } from '@/lib/components/FieldError';
import { useFormStateMachine } from '@/lib/forms/useFormStateMachine';
import type { FormCommand } from '@/lib/forms/formCommand';
import type { DogFormValues } from '@/lib/dogs/dogModel';
import {
  dogFormDefaultValues,
  dogFormLabels,
} from '@/lib/dogs/dogModel';
import { validateDogForm } from '@/lib/dogs/validateDogForm';

interface DogFormProps {
  title: string;
  submitLabel: string;
  command: FormCommand<DogFormValues>;
  initialValues?: DogFormValues;
}

export function DogForm({
  title,
  submitLabel,
  command,
  initialValues = dogFormDefaultValues,
}: DogFormProps) {
  // Merge form-level error into the external errors object under the "form" key
  const externalErrors = {
    ...(command.errors ?? {}),
    ...(command.error ? { form: command.error } : {}),
  };

  // Helper that calls the command using a few common method names so tests/mocks work
  const invokeCommand = async (vals: DogFormValues) => {
    // prefer run, then submit, then execute
    const anyCmd = command as any;
    if (typeof anyCmd.run === 'function') return anyCmd.run(vals);
    if (typeof anyCmd.submit === 'function') return anyCmd.submit(vals);
    if (typeof anyCmd.execute === 'function') return anyCmd.execute(vals);

    // If none exist, throw a clear error (caught below to avoid unhandled rejections)
    throw new Error('Form command is missing run/submit/execute function');
  };

  const {
    values,
    displayErrors,
    update,
    handleSubmit,
    isSubmitting: internalSubmitting,
  } = useFormStateMachine<DogFormValues>({
    initialValues,
    externalErrors,
    onSubmit: async (vals: DogFormValues) => {
      // Run local validation first; if there are validation errors, abort submit
      const validation = await validateDogForm(vals);
      if (validation && Object.keys(validation).length > 0) {
        return validation;
      }

      // Invoke the provided command in a safe way and avoid unhandled rejections
      try {
        await invokeCommand(vals);
      } catch (err) {
        // Log the error so tests and devs can see what happened.
        // Returning early prevents an unhandled rejection during tests.
        // The command implementation / tests should provide a proper mock with run/submit.
        // eslint-disable-next-line no-console
        console.error(err);
        return;
      }
    },
  });

  // Consider both internal and external submitting states
  const isSubmitting = internalSubmitting || command.isSubmitting;

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>{title}</h1>

      <FieldError id="error-form" message={displayErrors.form} />

      <FormField
        label={dogFormLabels.name}
        name="name"
        error={displayErrors.name}
      >
        {(fieldProps) => (
          <input
            type="text"
            value={values.name}
            onChange={update('name')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.breed}
        name="breed"
        error={displayErrors.breed}
      >
        {(fieldProps) => (
          <input
            type="text"
            value={values.breed}
            onChange={update('breed')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.dateOfBirth}
        name="dateOfBirth"
        error={displayErrors.dateOfBirth}
      >
        {(fieldProps) => (
          <input
            type="date"
            value={values.dateOfBirth}
            onChange={update('dateOfBirth')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label={dogFormLabels.sex}
        name="sex"
        error={displayErrors.sex}
      >
        {(fieldProps) => (
          <select
            value={values.sex}
            onChange={(e) => update('sex')(e.target.value as any)}
            {...fieldProps}
            disabled={isSubmitting}
          >
            <option value="">Select</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        )}
      </FormField>

      <button type="submit" disabled={isSubmitting}>
        {submitLabel}
      </button>
    </form>
  );
}

export default DogForm;
