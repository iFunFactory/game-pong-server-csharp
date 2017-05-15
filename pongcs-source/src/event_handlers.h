// PLEASE ADD YOUR EVENT HANDLER DECLARATIONS HERE.

#ifndef SRC_EVENT_HANDLERS_H_
#define SRC_EVENT_HANDLERS_H_

#include <funapi.h>

#include "pongcs_messages.pb.h"
#include "pongcs_object.h"
#include "pongcs_rpc_messages.pb.h"
#include "pongcs_types.h"


namespace pongcs {

void RegisterEventHandlers();

}  // namespace pongcs

#endif  // SRC_EVENT_HANDLERS_H_
