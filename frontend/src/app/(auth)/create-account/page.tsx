'use client';

import { useRouter } from 'next/navigation';
import { CreateAccountForm } from '@/components/account/CreateAccountForm';
import type { CreateAccountCommand } from '@/api/account/createAccount';
import { useApiCommand } from '@/lib/hooks/useApiCommand';

export default function CreateAccountPage() {
  const router = useRouter();

  const command = useApiCommand<CreateAccountCommand>(
    '/api/customers',
    () => router.push('/create-account/success')
  );

  return <CreateAccountForm command={command} />;
}
