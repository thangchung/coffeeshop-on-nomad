# coffeeshop-on-nomad

The .NET coffeeshop application runs on Nomad and Consul Connect

# Services

<table>
<thead>
    <td>No.</td>
    <td>Service Name</td>
    <td>URI</td>
</thead>
<tr>
    <td>1</td>
    <td>product-service</td>
    <td>http://localhost:5001 and http://localhost:15001</td>
</tr>
<tr>
    <td>2</td>
    <td>counter-service</td>
    <td>http://localhost:5002</td>
</tr>
<tr>
    <td>3</td>
    <td>barista-service</td>
    <td>http://localhost:5003</td>
</tr>
<tr>
    <td>4</td>
    <td>kitchen-service</td>
    <td>http://localhost:5004</td>
</tr>
<tr>
    <td>5</td>
    <td>reverse-proxy</td>
    <td>http://localhost:5000</td>
</tr>
<tr>
    <td>6</td>
    <td>signalr-web</td>
    <td>http://localhost:3000</td>
</tr>
<tr>
    <td>7</td>
    <td>datagen-app</td>
    <td></td>
</tr>
</table>
