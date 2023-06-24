import { Header } from './Components/Header/Header';
import { Contacts } from './Components/Contacts/Contacts';
import { Messages } from './Components/Messages/Messages';
import { Footer } from './Components/Footer/Footer';
import { useQuery, gql } from '@apollo/client';
import Login from './Components/Login/Login';

const TEST_QUERY = gql`
  query {
	test {
	  content
	}
  }
`;

const USER_API_KEY = 'wa_api_key';

function App(): JSX.Element {
	const { loading, error } = useQuery(TEST_QUERY);

	const apiKey = localStorage.getItem(USER_API_KEY);
	if (apiKey === null) {
		return <Login />;
	}

	if (loading)
		return <p>Loading...</p>;
	if (error)
		return <p>AAA</p>;

	return (
		<div className='container'>
			<Header />
			<Contacts />
			<Messages contactName='My Contact' />
			<Footer />
		</div>
	);
}

export default App;
