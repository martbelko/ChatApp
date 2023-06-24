import { gql } from '@apollo/client';

export function prepareQueryLogin(email: string, password: string) {
	return gql`
		query {
			test {
			content
			}
		}
  `;
}
