using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSSStar : MonoBehaviour
{
    //There is a priority queue OPEN that stores states {\displaystyle (J,s,h)}(J, s, h) or the nodes, where {\displaystyle J}J - node identificator (Dewey's notation is used to identify nodes, {\displaystyle \epsilon }\epsilon  is a root), {\displaystyle s\in \{L,S\}}s\in\{L,S\} - state of the node {\displaystyle J}J (L - the node is live, which means it's not solved yet and S - the node is solved), {\displaystyle h\in (-\infty ,\infty )}h\in(-\infty, \infty) - value of the solved node. Items in OPEN queue are sorted descending by their {\displaystyle h}h value. If more than one node has the same value of {\displaystyle h}h, a node left-most in the tree is chosen.
    // OPEN := { (e, L, inf)}
    // while true do   // repeat until stopped
    //     pop an element p = (J, s, h) from the head of the OPEN queue
    //     if J = e and s = S then
    //         STOP the algorithm and return h as a result
    //     else
    //         apply Gamma operator for p
    //
    // Gamma operator for p=(J, s, h) is defined in the following way:
    //if s = L then
    //    if J is a terminal node then
    //        (1.) add(J, S, min(h, value(J))) to OPEN
    //    else if J is a MIN node then
    //        (2.) add(J.1, L, h) to OPEN
    //    else
    //        (3.) for j=1..number_of_children(J) add(J.j, L, h) to OPEN
    //else
    //    if J is a MIN node then
    //        (4.) add(parent(J), S, h) to OPEN
    //             remove from OPEN all the states that are associated with the children of parent(J)
    //    else if is_last_child(J) then   // if J is the last child of parent(J)
    //        (5.) add(parent(J), S, h) to OPEN
    //    else
    //        (6.) add(parent(J).(k+1), L, h) to OPEN   // add state associated with the next child of parent(J) to OPEN
}