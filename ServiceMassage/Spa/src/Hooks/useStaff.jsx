import { useQuery } from '@tanstack/react-query';
import { personService } from '../services';

const QUERY_KEY = ['personals'];

export function useStaff() {
    // Assuming Role ID 3 is for Staff, based on typical seeding. 
    // If not, we might need to fetch Roles first or filter on client side.
    // Ideally, we should fetch by Role Name "Staff" if supported, or ID.
    // Sticking to getByRole(3) pending verification of Role IDs.
    // Update: Will default to fetching all and filtering if API supports it, 
    // but let's try getByRole if we are sure.
    // Actually, referencing AuthService, User=2. Admin=1 usually. Staff might be 3.
    // Let's rely on a more generic usePersons and filter in the UI or use a specific hook if we confirm ID.
    // For now, let's create a hook that fetches ALL persons and filters by RoleName 'Staff' if possible, 
    // OR uses the getByRole endpoint if we know the ID.

    // Safe bet: fetch all and filter client side for now to avoid ID mismatch until verified, 
    // OR use getByRole(3) and if empty handle it.

    // Actually, let's use a custom hook that checks for 'Staff' role name.
    return useQuery({
        queryKey: [...QUERY_KEY, 'staff'],
        queryFn: async () => {
            const res = await personService.getAll();
            // Filter for staff
            return Array.isArray(res) ? res.filter(p => p.roleName === 'Staff' || p.roleName === 'STAFF') : [];
        },
        staleTime: 5 * 60 * 1000,
    });
}
