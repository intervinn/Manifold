import useSWR from "swr"

// manifold api url
export const API_URL = import.meta.env.VITE_API_URL

export const fetcher = (...args: Parameters<typeof fetch>) => fetch(...args).then(res => res.json())

export function useAPI(endpoint: string) {
    const {data, error, isLoading} = useSWR(new URL(endpoint, API_URL), fetcher)

    return {
        data: data,
        error: error,
        isLoading: isLoading
    }
}