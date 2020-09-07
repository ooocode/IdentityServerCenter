import { useParams, useLocation, RouteComponentProps } from '@reach/router'
import qs from 'querystring'
export default function queryStringParser() {
    var location = useLocation();
    console.log(location)
    if (location.search.startsWith("?")) {
        location.search = location.search.slice(1)
    }
    var c = qs.parse(location.search);
    return c;
}