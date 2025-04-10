// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract Owner{

    address public owner;

        constructor(address _owner)
    {
        owner = _owner;
    }
    
    modifier onlyOwner() {
        require(msg.sender == owner, "Only the owner can perform this action.");
        _;
    }
}