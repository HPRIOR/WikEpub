class ValidateUrls {
    constructor(requestValidator) {
        this.requestValidator = requestValidator;
        this.nodesAreValid = new Event('nodesAreValid');
        this.nodesAreInvalid = new Event('nodesAreInvalid');
        document.addEventListener('inputChange', () => this.CheckNodesUponChange());
    }
    CheckNodesUponChange() {
        let nodes = document.getElementsByClassName("url-input");
        if (this.allNodesAreValid(nodes))
            document.dispatchEvent(this.nodesAreValid);
        else
            document.dispatchEvent(this.nodesAreInvalid);
    }
    NodeIsValid(node) {
        // check regex
        // make http request and check response
        return true;
    }
    allNodesAreValid(nodes) {
        let numberOfNodes = nodes.length;
        let numOfValidatedNodes = 0;
        for (var i = 0; i < numberOfNodes; i++) {
            if (this.NodeIsValid(nodes[i]))
                numberOfNodes++;
        }
        return numOfValidatedNodes === numberOfNodes;
    }
}
//# sourceMappingURL=ValidateUrls.js.map